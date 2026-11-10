import clsx from 'clsx';
import styles from './Text.module.scss';

const SectionTitle = ({ children, className = "", style}) => {
    return (
      <h2 className={clsx(styles.sectionTitle, className)} style={style}>
        {children}
      </h2>
    );
  };
  
  export default SectionTitle;
  