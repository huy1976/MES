import React from 'react';
import clsx from 'clsx';
import ReactSelect from 'react-select';
import styles from './Selection.module.scss';

const Select = ({ children, className = "", style, value, onChange, placeholder, ...props}) => {
  // Convert children options to react-select format
  const options = [];
  let defaultValue = null;
  
  // Process React children to extract options
  React.Children.forEach(children, (child) => {
    if (child && child.type === 'option') {
      const option = {
        value: child.props.value,
        label: child.props.children,
        isDisabled: child.props.disabled
      };
      
      options.push(option);
      
      // Set default value
      if (value === child.props.value) {
        defaultValue = option;
      }
    }
  });
  
  // Custom handler to make it work like native select
  const handleChange = (selectedOption) => {
    if (onChange) {
      // Simulate native select onChange event
      onChange({ target: { value: selectedOption ? selectedOption.value : '' } });
    }
  };
  
  const customStyles = {
    menu: (provided) => ({
      ...provided,
      marginTop: '4px', // Ensures dropdown appears below
      zIndex: 9999
    }),
    control: (provided) => ({
      ...provided,
      minHeight: '36px',
      borderRadius: '6px',
      border: '1px solid #767676',
      ...(style || {}) // Apply any custom style passed as prop
    }),
    placeholder: (provided) => ({
      ...provided,
      color: '#767676'
    })
  };
  
  // Get placeholder from disabled option if not provided directly
  let selectPlaceholder = placeholder;
  if (!selectPlaceholder) {
    // Try to find a disabled selected option to use as placeholder
    React.Children.forEach(children, (child) => {
      if (child && child.type === 'option' && child.props.disabled && child.props.selected) {
        selectPlaceholder = child.props.children;
      }
    });
  }
  
  return (
    <ReactSelect
      className={clsx(styles.reactSelect, className)}
      styles={customStyles}
      options={options}
      value={defaultValue}
      onChange={handleChange}
      menuPlacement="bottom" // Force menu to appear below
      menuPosition="fixed" // Ensure proper positioning
      isSearchable={false}
      placeholder={selectPlaceholder}
      isClearable={false}
      {...props}
    />
  );
};
  
export default Select;