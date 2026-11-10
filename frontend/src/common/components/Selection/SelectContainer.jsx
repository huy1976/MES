import React from 'react';
import clsx from 'clsx';
import styles from './Selection.module.scss';

const SelectContainer = ({ children, className = "", style}) => {
  // Check if the children contain a react-select component
  const hasReactSelect = React.Children.toArray(children).some(child => 
    child && child.type && child.type.name === 'Select'
  );

  // If using react-select, only render the Select component (not the dropdown icon)
  if (hasReactSelect) {
    const selectComponent = React.Children.toArray(children).find(child => 
      child && child.type && child.type.name === 'Select'
    );
    
    return (
      <div className={clsx(styles.selectContainer, className)} style={style}>
        {selectComponent}
      </div>
    );
  }
  
  // Otherwise render all children (for backwards compatibility)
  return <div className={clsx(styles.selectContainer, className)} style={style}>{children}</div>;
};
  
export default SelectContainer;